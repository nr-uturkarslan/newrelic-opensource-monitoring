package com.newrelic.javasecond.controller;

import com.newrelic.javasecond.dto.RequestDto;
import com.newrelic.javasecond.dto.ResponseDto;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequestMapping("java")
public class JavaSecondController {

    private final Logger logger = LoggerFactory.getLogger(JavaSecondController.class);

    @PostMapping()
    public ResponseEntity<ResponseDto> javaSecondMethod(
            @RequestBody RequestDto requestDto
    ) {
        logger.info("Java second method is triggered...");

        var responseDto = new ResponseDto();
        responseDto.setId(UUID.randomUUID().toString());
        responseDto.setValue(requestDto.getValue());
        responseDto.setTag(requestDto.getTag());

        var response = new ResponseEntity<>(responseDto, HttpStatus.OK);

        logger.info("Java second method is executed.");

        return response;
    }
}
